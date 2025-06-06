#
# Licensed to the Apache Software Foundation (ASF) under one
# or more contributor license agreements. See the NOTICE file
# distributed with this work for additional information
# regarding copyright ownership. The ASF licenses this file
# to you under the Apache License, Version 2.0 (the
# "License"); you may not use this file except in compliance
# with the License. You may obtain a copy of the License at
#
#   http://www.apache.org/licenses/LICENSE-2.0
#
# Unless required by applicable law or agreed to in writing,
# software distributed under the License is distributed on an
# "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
# KIND, either express or implied. See the License for the
# specific language governing permissions and limitations
# under the License.
#

# Unit tests still require boost
include(BoostMacros)
REQUIRE_BOOST_HEADERS()
set(BOOST_COMPONENTS filesystem thread unit_test_framework chrono)
REQUIRE_BOOST_LIBRARIES(BOOST_COMPONENTS)

include(ThriftMacros)

# Make sure gen-cpp files can be included
include_directories("${CMAKE_CURRENT_BINARY_DIR}")

# Create the thrift C++ test library
set(testgencpp_SOURCES
    gen-cpp/AnnotationTest_types.cpp
    gen-cpp/AnnotationTest_types.h
    gen-cpp/DebugProtoTest_types.cpp
    gen-cpp/DebugProtoTest_types.h
    gen-cpp/EnumTest_types.cpp
    gen-cpp/EnumTest_types.h
    gen-cpp/OptionalRequiredTest_types.cpp
    gen-cpp/OptionalRequiredTest_types.h
    gen-cpp/Recursive_types.cpp
    gen-cpp/Recursive_types.h
    gen-cpp/ThriftTest_types.cpp
    gen-cpp/ThriftTest_types.h
    gen-cpp/OneWayTest_types.h
    gen-cpp/OneWayService.cpp
    gen-cpp/OneWayService.h
    gen-cpp/TypedefTest_types.cpp
    gen-cpp/TypedefTest_types.h
    gen-cpp/Thrift5272_types.cpp
    gen-cpp/Thrift5272_types.h
    ThriftTest_extras.cpp
    DebugProtoTest_extras.cpp
)

add_library(testgencpp STATIC ${testgencpp_SOURCES})

set(testgencpp_cob_SOURCES
    gen-cpp/ChildService.cpp
    gen-cpp/ChildService.h
    gen-cpp/EmptyService.cpp
    gen-cpp/EmptyService.h
    gen-cpp/ParentService.cpp
    gen-cpp/ParentService.h
    gen-cpp/proc_types.cpp
    gen-cpp/proc_types.h
)
add_library(testgencpp_cob STATIC ${testgencpp_cob_SOURCES})

add_executable(Benchmark Benchmark.cpp)
target_link_libraries(Benchmark testgencpp)
target_link_libraries(Benchmark thrift)
add_test(NAME Benchmark COMMAND Benchmark)
target_link_libraries(Benchmark testgencpp)

set(UnitTest_SOURCES
    UnitTestMain.cpp
    OneWayHTTPTest.cpp
    TMemoryBufferTest.cpp
    TBufferBaseTest.cpp
    Base64Test.cpp
    ToStringTest.cpp
    TypedefTest.cpp
    TServerSocketTest.cpp
    TServerTransportTest.cpp
    ThrifttReadCheckTests.cpp
    TUuidTest.cpp
    Thrift5272.cpp
)

add_executable(UnitTests ${UnitTest_SOURCES})
target_link_libraries(UnitTests testgencpp ${Boost_LIBRARIES})
target_link_libraries(UnitTests thrift)
add_test(NAME UnitTests COMMAND UnitTests)
if(MSVC)
    # Disable C4503: decorated name length exceeded, name was truncated
    # 'insanity' results in very long decorated names
    set_property( TARGET UnitTests APPEND_STRING PROPERTY COMPILE_FLAGS /wd4503 )
endif()

# Test the THRIFT_TUUID_SUPPORT_BOOST_UUID compiler directive globally set on the target
add_executable(UnitTestsUuid
    UnitTestMain.cpp
    TUuidTestBoost.cpp
)
target_link_libraries(UnitTestsUuid testgencpp ${Boost_LIBRARIES})
target_link_libraries(UnitTestsUuid thrift)
target_compile_definitions(UnitTestsUuid PUBLIC THRIFT_TUUID_SUPPORT_BOOST_UUID)
add_test(NAME UnitTestsUuid COMMAND UnitTestsUuid)

# Test not setting the THRIFT_TUUID_SUPPORT_BOOST_UUID compiler directive as with the test above.
# The test does set the directive before including the thrift header to test the behaviour
add_executable(UnitTestsUuidNoDirective
    UnitTestMain.cpp
    TUuidTestBoostNoDirective.cpp
)
target_link_libraries(UnitTestsUuidNoDirective testgencpp ${Boost_LIBRARIES})
target_link_libraries(UnitTestsUuidNoDirective thrift)
add_test(NAME UnitTestsUuidNoDirective COMMAND UnitTestsUuidNoDirective)

set( TInterruptTest_SOURCES
     TSocketInterruptTest.cpp
     TSSLSocketInterruptTest.cpp
)
if (WIN32)
    list(APPEND TInterruptTest_SOURCES
        TPipeInterruptTest.cpp
    )
endif()
add_executable(TInterruptTest ${TInterruptTest_SOURCES})
target_link_libraries(TInterruptTest
    testgencpp
    ${Boost_LIBRARIES}
)
target_link_libraries(TInterruptTest thrift)
if (NOT MSVC AND NOT ${CMAKE_SYSTEM_NAME} MATCHES "Darwin" AND NOT ${CMAKE_SYSTEM_NAME} MATCHES "OpenBSD" AND NOT MINGW)
    target_link_libraries(TInterruptTest -lrt)
endif ()
add_test(NAME TInterruptTest COMMAND TInterruptTest -- "${CMAKE_CURRENT_SOURCE_DIR}/../../../test/keys")

add_executable(TServerIntegrationTest TServerIntegrationTest.cpp)
target_link_libraries(TServerIntegrationTest
    testgencpp_cob
    ${Boost_LIBRARIES}
)
target_link_libraries(TServerIntegrationTest thrift)
if (NOT MSVC AND NOT ${CMAKE_SYSTEM_NAME} MATCHES "Darwin" AND NOT ${CMAKE_SYSTEM_NAME} MATCHES "OpenBSD" AND NOT MINGW)
    target_link_libraries(TServerIntegrationTest -lrt)
endif ()
add_test(NAME TServerIntegrationTest COMMAND TServerIntegrationTest)

if(WITH_ZLIB)
include_directories(SYSTEM "${ZLIB_INCLUDE_DIRS}")
add_executable(TransportTest TransportTest.cpp)
target_link_libraries(TransportTest
    testgencpp
    ${Boost_LIBRARIES}
    ${ZLIB_LIBRARIES}
)
target_link_libraries(TransportTest thrift)
target_link_libraries(TransportTest thriftz)
add_test(NAME TransportTest COMMAND TransportTest)

add_executable(ZlibTest ZlibTest.cpp)
target_link_libraries(ZlibTest
    testgencpp
    ${Boost_LIBRARIES}
    ${ZLIB_LIBRARIES}
)
target_link_libraries(ZlibTest thrift)
target_link_libraries(ZlibTest thriftz)
add_test(NAME ZlibTest COMMAND ZlibTest)
endif(WITH_ZLIB)

add_executable(AnnotationTest AnnotationTest.cpp)
target_link_libraries(AnnotationTest
    testgencpp
    ${Boost_LIBRARIES}
)
target_link_libraries(AnnotationTest thrift)
add_test(NAME AnnotationTest COMMAND AnnotationTest)

add_executable(EnumTest EnumTest.cpp)
target_link_libraries(EnumTest
    testgencpp
    ${Boost_LIBRARIES}
)
target_link_libraries(EnumTest thrift)
add_test(NAME EnumTest COMMAND EnumTest)

if(HAVE_GETOPT_H)
add_executable(TFileTransportTest TFileTransportTest.cpp)
target_link_libraries(TFileTransportTest
    testgencpp
    ${Boost_LIBRARIES}
)
target_link_libraries(TFileTransportTest thrift)
add_test(NAME TFileTransportTest COMMAND TFileTransportTest)
endif()

add_executable(TFDTransportTest TFDTransportTest.cpp)
target_link_libraries(TFDTransportTest
    ${Boost_LIBRARIES}
)
target_link_libraries(TFDTransportTest thrift)
add_test(NAME TFDTransportTest COMMAND TFDTransportTest)

add_executable(TPipedTransportTest TPipedTransportTest.cpp)
target_link_libraries(TPipedTransportTest
    ${Boost_LIBRARIES}
)
target_link_libraries(TPipedTransportTest thrift)
add_test(NAME TPipedTransportTest COMMAND TPipedTransportTest)

set(AllProtocolsTest_SOURCES
    AllProtocolTests.cpp
    AllProtocolTests.tcc
    GenericHelpers.h
)

add_executable(AllProtocolsTest ${AllProtocolsTest_SOURCES})
target_link_libraries(AllProtocolsTest
    testgencpp
    ${Boost_LIBRARIES}
)
target_link_libraries(AllProtocolsTest thrift)
add_test(NAME AllProtocolsTest COMMAND AllProtocolsTest)

# The debug run-time in Windows asserts on isprint() with negative inputs
if (NOT MSVC OR (MSVC AND CMAKE_BUILD_TYPE EQUAL "DEBUG"))
    add_executable(DebugProtoTest DebugProtoTest.cpp)
    target_link_libraries(DebugProtoTest
        testgencpp
        ${Boost_LIBRARIES}
    )
    target_link_libraries(DebugProtoTest thrift)
    add_test(NAME DebugProtoTest COMMAND DebugProtoTest)
endif()

add_executable(JSONProtoTest JSONProtoTest.cpp)
target_link_libraries(JSONProtoTest
    testgencpp
    ${Boost_LIBRARIES}
)
target_link_libraries(JSONProtoTest thrift)
add_test(NAME JSONProtoTest COMMAND JSONProtoTest)

add_executable(OptionalRequiredTest OptionalRequiredTest.cpp)
target_link_libraries(OptionalRequiredTest
    testgencpp
    ${Boost_LIBRARIES}
)
target_link_libraries(OptionalRequiredTest thrift)
add_test(NAME OptionalRequiredTest COMMAND OptionalRequiredTest)

add_executable(RecursiveTest RecursiveTest.cpp)
target_link_libraries(RecursiveTest
    testgencpp
    ${Boost_LIBRARIES}
)
target_link_libraries(RecursiveTest thrift)
add_test(NAME RecursiveTest COMMAND RecursiveTest)

add_executable(SpecializationTest SpecializationTest.cpp)
target_link_libraries(SpecializationTest
    testgencpp
    ${Boost_LIBRARIES}
)
target_link_libraries(SpecializationTest thrift)
add_test(NAME SpecializationTest COMMAND SpecializationTest)

set(concurrency_test_SOURCES
    concurrency/Tests.cpp
    concurrency/ThreadFactoryTests.h
    concurrency/ThreadManagerTests.h
    concurrency/TimerManagerTests.h
)
add_executable(concurrency_test ${concurrency_test_SOURCES})
target_link_libraries(concurrency_test thrift)
add_test(NAME concurrency_test COMMAND concurrency_test)

set(link_test_SOURCES
    link/LinkTest.cpp
    gen-cpp/ParentService.h
    link/TemplatedService1.cpp
    link/TemplatedService2.cpp
)

add_executable(link_test ${link_test_SOURCES})
target_link_libraries(link_test testgencpp_cob)
target_link_libraries(link_test thrift)
target_link_libraries(link_test testgencpp)
add_test(NAME link_test COMMAND link_test)

if(WITH_LIBEVENT)
    set(processor_test_SOURCES
        processor/ProcessorTest.cpp
        processor/EventLog.cpp
        processor/ServerThread.cpp
        processor/EventLog.h
        processor/Handlers.h
        processor/ServerThread.h
    )
    add_executable(processor_test ${processor_test_SOURCES})
    target_link_libraries(processor_test
        testgencpp_cob
        ${Boost_LIBRARIES}
    )
    target_link_libraries(processor_test thriftnb)
    add_test(NAME processor_test COMMAND processor_test)

    set(TNonblockingServerTest_SOURCES TNonblockingServerTest.cpp)
    add_executable(TNonblockingServerTest ${TNonblockingServerTest_SOURCES})
    include_directories(${LIBEVENT_INCLUDE_DIRS})
    target_link_libraries(TNonblockingServerTest
        testgencpp_cob
        ${Boost_LIBRARIES}
    )
    target_link_libraries(TNonblockingServerTest thriftnb)
    add_test(NAME TNonblockingServerTest COMMAND TNonblockingServerTest)

    if(OPENSSL_FOUND AND WITH_OPENSSL)
      set(TNonblockingSSLServerTest_SOURCES TNonblockingSSLServerTest.cpp)
      add_executable(TNonblockingSSLServerTest ${TNonblockingSSLServerTest_SOURCES})
      include_directories(${LIBEVENT_INCLUDE_DIRS})
      target_link_libraries(TNonblockingSSLServerTest
        testgencpp_cob
        ${Boost_LIBRARIES}
      )
      target_link_libraries(TNonblockingSSLServerTest thriftnb)
      add_test(NAME TNonblockingSSLServerTest COMMAND TNonblockingSSLServerTest -- "${CMAKE_CURRENT_SOURCE_DIR}/../../../test/keys")
    endif(OPENSSL_FOUND AND WITH_OPENSSL)
endif()

if(OPENSSL_FOUND AND WITH_OPENSSL)
add_executable(OpenSSLManualInitTest OpenSSLManualInitTest.cpp)
target_link_libraries(OpenSSLManualInitTest
    ${OPENSSL_LIBRARIES}
    ${Boost_LIBRARIES}
)
target_link_libraries(OpenSSLManualInitTest thrift)
add_test(NAME OpenSSLManualInitTest COMMAND OpenSSLManualInitTest)

add_executable(SecurityTest SecurityTest.cpp)
target_link_libraries(SecurityTest
    testgencpp
    ${Boost_LIBRARIES}
)
target_link_libraries(SecurityTest thrift)
if (NOT MSVC AND NOT ${CMAKE_SYSTEM_NAME} MATCHES "Darwin" AND NOT ${CMAKE_SYSTEM_NAME} MATCHES "OpenBSD" AND NOT MINGW)
    target_link_libraries(SecurityTest -lrt)
endif ()
add_test(NAME SecurityTest COMMAND SecurityTest -- "${CMAKE_CURRENT_SOURCE_DIR}/../../../test/keys")

add_executable(SecurityFromBufferTest SecurityFromBufferTest.cpp)
target_link_libraries(SecurityFromBufferTest
    testgencpp
    ${Boost_LIBRARIES}
)
target_link_libraries(SecurityFromBufferTest thrift)
if (NOT MSVC AND NOT ${CMAKE_SYSTEM_NAME} MATCHES "Darwin" AND NOT ${CMAKE_SYSTEM_NAME} MATCHES "OpenBSD" AND NOT MINGW)
    target_link_libraries(SecurityFromBufferTest -lrt)
endif ()
add_test(NAME SecurityFromBufferTest COMMAND SecurityFromBufferTest -- "${CMAKE_CURRENT_SOURCE_DIR}/../../../test/keys")

endif()

if(WITH_QT5)
    add_subdirectory(qt)
endif()

#
# Common thrift code generation rules
#
# files from /test
#

add_custom_command(OUTPUT gen-cpp/AnnotationTest_constants.cpp
                          gen-cpp/AnnotationTest_constants.h
                          gen-cpp/AnnotationTest_types.cpp
                          gen-cpp/AnnotationTest_types.h
                          gen-cpp/foo_service.cpp
                          gen-cpp/foo_service.h
    COMMAND ${THRIFT_COMPILER} --gen cpp ${PROJECT_SOURCE_DIR}/test/AnnotationTest.thrift
)

add_custom_command(OUTPUT gen-cpp/DebugProtoTest_types.cpp gen-cpp/DebugProtoTest_types.h gen-cpp/EmptyService.cpp gen-cpp/EmptyService.h
    COMMAND ${THRIFT_COMPILER} --gen cpp ${PROJECT_SOURCE_DIR}/test/DebugProtoTest.thrift
)

add_custom_command(OUTPUT gen-cpp/EnumTest_types.cpp gen-cpp/EnumTest_types.h
    COMMAND ${THRIFT_COMPILER} --gen cpp ${PROJECT_SOURCE_DIR}/test/EnumTest.thrift
)

add_custom_command(OUTPUT gen-cpp/TypedefTest_types.cpp gen-cpp/TypedefTest_types.h
    COMMAND ${THRIFT_COMPILER} --gen cpp ${PROJECT_SOURCE_DIR}/test/TypedefTest.thrift
)

add_custom_command(OUTPUT gen-cpp/OptionalRequiredTest_types.cpp gen-cpp/OptionalRequiredTest_types.h
    COMMAND ${THRIFT_COMPILER} --gen cpp ${PROJECT_SOURCE_DIR}/test/OptionalRequiredTest.thrift
)

add_custom_command(OUTPUT gen-cpp/Recursive_types.cpp gen-cpp/Recursive_types.h
    COMMAND ${THRIFT_COMPILER} --gen cpp ${PROJECT_SOURCE_DIR}/test/Recursive.thrift
)

add_custom_command(OUTPUT gen-cpp/Service.cpp gen-cpp/StressTest_types.cpp
    COMMAND ${THRIFT_COMPILER} --gen cpp ${PROJECT_SOURCE_DIR}/test/StressTest.thrift
)

add_custom_command(OUTPUT gen-cpp/SecondService.cpp gen-cpp/ThriftTest_constants.cpp gen-cpp/ThriftTest.cpp gen-cpp/ThriftTest_types.cpp gen-cpp/ThriftTest_types.h
    COMMAND ${THRIFT_COMPILER} --gen cpp ${PROJECT_SOURCE_DIR}/test/ThriftTest.thrift
)

# files from /lib/cpp/test

add_custom_command(OUTPUT gen-cpp/OneWayService.cpp gen-cpp/OneWayTest_types.h gen-cpp/OneWayService.h
    COMMAND ${THRIFT_COMPILER} --gen cpp ${CMAKE_CURRENT_SOURCE_DIR}/OneWayTest.thrift
)

add_custom_command(OUTPUT gen-cpp/Thrift5272_types.cpp gen-cpp/Thrift5272_types.h
    COMMAND ${THRIFT_COMPILER} --gen cpp ${CMAKE_CURRENT_SOURCE_DIR}/Thrift5272.thrift
)

add_custom_command(OUTPUT gen-cpp/ChildService.cpp gen-cpp/ChildService.h gen-cpp/ParentService.cpp gen-cpp/ParentService.h gen-cpp/proc_types.cpp gen-cpp/proc_types.h
    COMMAND ${THRIFT_COMPILER} --gen cpp:templates,cob_style ${CMAKE_CURRENT_SOURCE_DIR}/processor/proc.thrift
)
